using Newtonsoft.Json;
using RegServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RegServices
{
    // This not used : is replaced with MobRegService.cs

    public class Service
    {
        public bool HasError { get { return !string.IsNullOrEmpty(lastError); } }
        public string Error { get { return lastError; } }
        public bool IsBusy { get { return isBusy; } }
		public string URL { get { return baseUrl; } set {baseUrl = value; }}

        private string lastError;
        private bool isBusy;
        private HttpWebRequest request;
        private byte[] data;
        private string respString;
        private string baseUrl;
        
        public Service(string baseUrl="")
        {
            isBusy = false;
            if (string.IsNullOrEmpty(baseUrl))
                this.baseUrl = "http://mobrestservice.proapps.be/RestService/";
            else this.baseUrl = baseUrl;  
        }

        public void Load(LoadSyncInput input, Action<RestLoadResult> whenCompleted)
        {
             ProcessRestRequest<LoadSyncInput, RestLoadResult>("Load", input, whenCompleted);
        }

        public void LoginUser(LoginUserInput input, Action<LoginResult> whenCompleted)
        {
            ProcessRestRequest<LoginUserInput, LoginResult>("LoginUser", input, whenCompleted);
        }

		public void GetPlanning(GetPlanningInput input, Action<PlanningResult> whenCompleted)
		{
			ProcessRestRequest<GetPlanningInput, PlanningResult> ("GetPlanning", input, whenCompleted);
		}

		public void GetRegistrations(GetRegistrationInput input, Action<RegResult> whenCompleted)
		{
			ProcessRestRequest<GetRegistrationInput, RegResult> ("GetRegistrations", input, whenCompleted);
		}

		public void UpdateStatusForGroup(UpdateStatusInput input, Action<StatusResult> whenCompleted)
		{
			ProcessRestRequest<UpdateStatusInput, StatusResult> ("UpdateStatusForGroup", input, whenCompleted);
		}

		public void SaveRegistrations (SaveRegistrationInput input, Action<StatusResult> whenCompleted)
		{
			ProcessRestRequest<SaveRegistrationInput, StatusResult> ("SaveRegistrations", input, whenCompleted);
		}

		public void SavePlanning (SavePlanningInput input, Action<StatusResult> whenCompleted)
		{
			ProcessRestRequest<SavePlanningInput, StatusResult> ("SavePlanning", input, whenCompleted);
		}

        //public void Load(LoadSyncInput input, Action<RestLoadResult> whenCompleted)
        //{
        //    string postData = JsonConvert.SerializeObject(input);

        //    ProcessRestRequest("Load", postData, (response) => 
        //    {
        //        RestLoadResult result = null;
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(response)) result = JsonConvert.DeserializeObject<RestLoadResult>(respString);
        //        }
        //        catch(Exception ex) { lastError = "Deserializing json : " + ex.Message; }

        //        whenCompleted.Invoke(result);
        //        isBusy = false;
        //    });
        //}

        //public void LoginUser(LoginUserInput input, Action<LoginResult> whenCompleted)
        //{
        //    string postData = JsonConvert.SerializeObject(input);

        //    ProcessRestRequest("LoginUser", postData, (response) =>
        //    {
        //        LoginResult result = null;
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(response)) result = JsonConvert.DeserializeObject<LoginResult>(respString);
        //        }
        //        catch (Exception ex) { lastError = "Deserializing json : " + ex.Message; }

        //        whenCompleted.Invoke(result);
        //        isBusy = false;
        //    });
        //}

        private void ProcessRestRequest<T,V>(string methodName, T input, Action<V>whenCompleted)
        {
            string postData = JsonConvert.SerializeObject(input);

            ProcessRestRequest(methodName, postData, (response) =>
            {
                V result = default(V);
                try
                {
                    if (!string.IsNullOrEmpty(response)) result = JsonConvert.DeserializeObject<V>(respString);
                }
                catch (Exception ex) { lastError = "Deserializing json : " + ex.Message; }

                whenCompleted.Invoke(result);
                isBusy = false;
            });

        }


        private void ProcessRestRequest(string methodName, string webrequestInput, Action<string> requestCompleted)
        {
            try
            {
                lastError = string.Empty;
                if (isBusy) throw new Exception("Service is busy");
                isBusy = true;

                request = (HttpWebRequest)WebRequest.Create(baseUrl + methodName);
                request.Method = "POST";

                data = Encoding.UTF8.GetBytes(webrequestInput);

                request.ContentType = "application/json;charset=UTF-8";
                request.Accept = "application/json";
                request.Headers["ContentLength"] = data.Length.ToString();

                //System.Collections.Specialized.NameValueCollection n = new NameValueCollection();
                //n.Add("application-key", "sadkl");
                //request.Headers.Add(n);

                //await Task.Factory.FromAsync(request.BeginGetRequestStream(BeginGetRequestStreamCallBack2, data), async ar =>
                //{
                //    await Task.Factory.FromAsync(request.BeginGetResponse(BeginGetResponseStreamCallBack2, null), ar2 =>
                //    {
                //        requestCompleted.Invoke();
                //    });


                //});

                if (request.Method != "GET")
                {
                    request.BeginGetRequestStream(BeginGetRequestStreamCallBack2, requestCompleted);
                    return;
                }
                else request.BeginGetResponse(BeginGetResponseStreamCallBack2, requestCompleted);

            }
            catch (WebException ex)
            {
                lastError = ex.Message;
                isBusy = false;
            }

        }

        private void BeginGetRequestStreamCallBack2(IAsyncResult ar)
        {
            try {
                using (Stream requestStream = request.EndGetRequestStream(ar))
                {
                    requestStream.Write(data, 0, data.Length);
                }
                request.BeginGetResponse(BeginGetResponseStreamCallBack2, ar.AsyncState);
            }
            catch(Exception ex)
            {
                lastError = ex.Message;
                Action<string> action = ar.AsyncState as Action<string>;
                action.Invoke("");
            }
        }


        private void BeginGetResponseStreamCallBack2(IAsyncResult ar)
        {
            using (WebResponse response = request.EndGetResponse(ar))
            {
                // You can analyse headers here
                // string s = response.Headers["application-response"];

                // Do something with response
                using (Stream responseStream = response.GetResponseStream())
                {
                    try
                    {
                        responseStream.ReadTimeout = 1000 * 120;
                    }
                    catch (Exception ex) { }

                    long length = response.ContentLength;
                    int len = (int)length;
                    byte[] resp = new byte[len + 1];

                    int totalRead = 0;
                    while (totalRead != len)
                        totalRead += responseStream.Read(resp, totalRead, len - totalRead);

                    respString = Encoding.UTF8.GetString(resp, 0, len);
                }
            }
            Action<string> action = ar.AsyncState as Action<string>;
            action.Invoke(respString);
        }

        #region TestCode
        /*
        string method;
        string lbStatus;
        public void Execute()
        {
            ////Trust all certificates
            //System.Net.ServicePointManager.ServerCertificateValidationCallback =
            //    ((sender, certificate, chain, sslPolicyErrors) =>{
            //        string s = certificate.Subject;
            //        return true;
            //    });
            

            //method = "ValidateClientRevision";
            method = "Load";
            //string method = "Synchronize";
            //method = "GetPlanning";
            //string method = "GetRegistrations";
            //string method = "LoginUser";
            //string method = "RegisterClient";
            string postData = "";

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:57821/RestService/ValidateClientRevision");
            //request = (HttpWebRequest)WebRequest.Create("http://localhost:7100/RestService/RestService/" + method);
            request = (HttpWebRequest)WebRequest.Create("http://mobrestservice.proapps.be/RestService/" + method);

            request.Method = "POST";
            //request.Headers.Add("Authorization: OAuth " + accessToken);
            if (method == "RegisterClient")
            {
                ClientRegistrationInput rev = new ClientRegistrationInput() { installationID = 1, clientID = "0cd19f51-3d46-485a-83a4-edec5ababb10", userID = null };
                postData = JsonConvert.SerializeObject(rev);
            }
            if (method == "ValidateClientRevision")
            {
                ClientRevisionInput rev = new ClientRevisionInput() { major = 1, minor = 0, subminor = 0, lower = 0, languageID = "nl-BE" };
                postData = JsonConvert.SerializeObject(rev);
            }
            if (method == "Load")
            {
                LoadSyncInput inputObject = new LoadSyncInput() { installationID = 1, userID = "ff38aa48-20cd-40fd-b90b-a106ec9e3b13", loadSyncList = "Status|MobUser|RegistrationType", parameters = "" };
                postData = JsonConvert.SerializeObject(inputObject);
            }
            if (method == "Synchronize")
            {
                LoadSyncInput inputObject = new LoadSyncInput() { installationID = 1, userID = "ff38aa48-20cd-40fd-b90b-a106ec9e3b13", loadSyncList = "MobUser=0|Car=5|Label=0|Article=0|ArticleGroup=0", parameters = "" };
                postData = JsonConvert.SerializeObject(inputObject);
            }
            if (method == "GetPlanning")
            {
                GetPlanningInput inputObject = new GetPlanningInput() { installationID = 1, userID = "ff38aa48-20cd-40fd-b90b-a106ec9e3b13", carID = "1ABC001" };
                postData = JsonConvert.SerializeObject(inputObject);
            }
            if (method == "GetRegistrations")
            {
                GetRegistrationInput inputObject = new GetRegistrationInput() { installationID = 1, userID = "ff38aa48-20cd-40fd-b90b-a106ec9e3b13", planningID = "1e32663f-127d-4e92-916b-8bf58c5cb8ee" };
                postData = JsonConvert.SerializeObject(inputObject);
            }
            if (method == "LoginUser")
            {
                LoginUserInput inputObject = new LoginUserInput() { userName = "Chris", password = "test", installationID = 1, carID = "1ABC001", deviceLangID = "nl-BE", loginHistoryID = 0, login = true };
                postData = JsonConvert.SerializeObject(inputObject);
            }


            data = Encoding.UTF8.GetBytes(postData);

            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "application/json;charset=UTF-8";
            request.Accept = "application/json";
            request.Headers["ContentLength"] = data.Length.ToString();

            //System.Collections.Specialized.NameValueCollection n = new NameValueCollection();
            //n.Add("application-key", "sadkl");
            //request.Headers.Add(n);


            if (request.Method != "GET")
            {

                //using (Stream requestStream = request.GetRequestStream())
                //{
                //    requestStream.Write(data, 0, data.Length);
                //}
                //done = false;
                request.BeginGetRequestStream(BeginGetRequestStreamCallBack, data);
                //while (!done) { }
                //ProcessResult();

                return;
                //Stream requestStream = await request.GetRequestStreamAsync();
                //await requestStream.WriteAsync(data, 0, data.Length);
                //requestStream.Dispose();
                //requestStream = null;
            }

            try
            {
                //done = false;
                request.BeginGetResponse(BeginGetResponseStreamCallBack, data);
                //while (!done) { }
                //ProcessResult();


            }
            catch (WebException ex)
            {
                // Handle error
                lbStatus = ex.Message;
            }
        }

        private void ProcessResult()
        {
            if (method == "RegisterClient")
            {
                ClientRegResult result = JsonConvert.DeserializeObject<ClientRegResult>(respString);
                lbStatus = result.status;
            }
            if (method == "LoginUser")
            {
                LoginResult result = JsonConvert.DeserializeObject<LoginResult>(respString);
                lbStatus = result.status;
            }
            if (method == "Load")
            {
                RestLoadResult loadResult = JsonConvert.DeserializeObject<RestLoadResult>(respString);
                lbStatus = loadResult.status;
            }
            if (method == "Synchronize")
            {
                RestSyncResult result = JsonConvert.DeserializeObject<RestSyncResult>(respString);
                lbStatus = result.status;
            }
            if (method == "ValidateClientRevision")
            {
                respString = "{\"language\":\"en-US\",\"status\":\"OK\",\"statusCode\":0,\"version\":1}";

                StatusResult test = new StatusResult();
                string jsonText = JsonConvert.SerializeObject(test);

                StatusResult statusResult;

                //result = JsonConvert.DeserializeObject<StatusResult>(respString);

                statusResult = JsonConvert.DeserializeObject<StatusResult>(jsonText);



                //lbStatus = result.status;
            }
            if (method == "GetPlanning")
            {
                PlanningResult result = JsonConvert.DeserializeObject<PlanningResult>(respString);
                lbStatus = result.status;
            }
            if (method == "GetRegistrations")
            {
                RegResult result = JsonConvert.DeserializeObject<RegResult>(respString);
                lbStatus = result.status;
            }

        }

        private void BeginGetRequestStreamCallBack(IAsyncResult ar)
        {
            using (Stream requestStream = request.EndGetRequestStream(ar))
            {
                requestStream.Write(data, 0, data.Length);
            }
            request.BeginGetResponse(BeginGetResponseStreamCallBack, data);
        }


        private void BeginGetResponseStreamCallBack(IAsyncResult ar)
        {
            using (WebResponse response = request.EndGetResponse(ar))
            {
                // You can analyse headers here
                // string s = response.Headers["application-response"];

                // Do something with response
                using (Stream responseStream = response.GetResponseStream())
                {
                    try
                    {
                        responseStream.ReadTimeout = 1000 * 120;
                    }
                    catch (Exception ex) { }

                    long length = response.ContentLength;
                    int len = (int)length;
                    byte[] resp = new byte[len + 1];

                    int totalRead = 0;
                    while (totalRead != len)
                        totalRead += responseStream.Read(resp, totalRead, len - totalRead);

                    respString = Encoding.UTF8.GetString(resp, 0, len);
                }
            }
            //done = true;
            ProcessResult();

        }
        */
        #endregion



    }
}
