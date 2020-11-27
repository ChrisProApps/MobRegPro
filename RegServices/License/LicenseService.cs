using System;
using System.Threading.Tasks;
using RegServices.Data;

namespace RegServices
{
	public class LicenseService : HttpService
	{
		public LicenseService (string url) : base(url)
		{
		}

		public async Task<ModuleResult> GetRevisionAsync()
		{
			return await ProcessAsync<Object, ModuleResult> ("GetRevision", null, new ModuleResult());
		}

		public async Task<LicenseResult> ExecuteAsync (LicenseRequest input)
		{
			return await ProcessAsync<LicenseRequest, LicenseResult> ("Execute", input, new LicenseResult());
		}
	}
}

