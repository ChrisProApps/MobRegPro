using System;
using System.Threading.Tasks;
using RegServices.Data;

namespace RegServices
{
	public class MobRegService : HttpService
	{
		public MobRegService (string url) : base(url)
		{
		}

		public async Task<RestLoadResult> LoadAsync(LoadSyncInput input)
		{
			return await ProcessAsync<LoadSyncInput, RestLoadResult> ("Load", input, new RestLoadResult());
		}

		public async Task<RestSyncResult> SynchronizeAsync(LoadSyncInput input)
		{
			return await ProcessAsync<LoadSyncInput, RestSyncResult> ("Synchronize", input, new RestSyncResult());
		}

		public async Task<LoginResult> LoginUserAsync(LoginUserInput input)
		{
			return await ProcessAsync<LoginUserInput, LoginResult>("LoginUser", input, new LoginResult());
		}

		public async Task<PlanningResult> GetPlanningAsync(GetPlanningInput input)
		{
			return await ProcessAsync<GetPlanningInput, PlanningResult> ("GetPlanning", input, new PlanningResult());
		}

		public async Task<RegResult> GetRegistrationsAsync(GetRegistrationInput input)
		{
			return await ProcessAsync<GetRegistrationInput, RegResult> ("GetRegistrations", input, new RegResult());
		}

		public async Task<StatusResult> UpdateStatusForGroupAsync(UpdateStatusInput input)
		{
			return await ProcessAsync<UpdateStatusInput, StatusResult> ("UpdateStatusForGroup", input, new StatusResult());
		}

		public async Task<StatusResult> SaveRegistrationsAsync (SaveRegistrationInput input)
		{
			return await ProcessAsync<SaveRegistrationInput, StatusResult> ("SaveRegistrations", input, new StatusResult());
		}

		public async Task<StatusResult> SavePlanningAsync (SavePlanningInput input)
		{
			return await ProcessAsync<SavePlanningInput, StatusResult> ("SavePlanning", input, new StatusResult());
		}

		public async Task<ModuleResult> GetRevisionAsync()
		{
			return await ProcessAsync<Object, ModuleResult> ("GetRevision", null, new ModuleResult());
		}

		public async Task<ValidatePlanningResult> ValidatePlanningAsync (ValidatePlanningInput input)
		{
			return await ProcessAsync<ValidatePlanningInput, ValidatePlanningResult> ("ValidatePlanning", input, new ValidatePlanningResult());
		}
	}
}

