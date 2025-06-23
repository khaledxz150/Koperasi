namespace Models.ViewModels.User.Response
{
    public class RegistrationStepsResponse
    {
        public List<RegistrationStepInfo> Steps { get; set; } = new List<RegistrationStepInfo>();
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }
        public string CurrentStepTitle { get; set; }
        public string CurrentStepDescription { get; set; }
    }
}
