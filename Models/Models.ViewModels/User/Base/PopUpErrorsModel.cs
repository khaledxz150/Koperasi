using Models.Enums.System;

namespace Models.ViewModels.User.Base
{
    public class PopUpErrorsModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public ValidationPopupTypesEnum ValidationPopupTypesEnum { get; set; } = ValidationPopupTypesEnum.None;
    }
}
