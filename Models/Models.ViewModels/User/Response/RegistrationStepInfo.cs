using static Models.Enums.User.RegistrationStatusEnum;

namespace Models.ViewModels.User.Response
{
    public class RegistrationStepInfo
    {
        public int StepNumber { get; set; }
        public RegistrationStatus Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
    }
}
