﻿using Models.ViewModels.User.Base;

namespace Models.ViewModels.User.Response
{
    public class CompletionResponse : BaseResponse
    {
        public bool IsComplete { get; set; }
        public DateTime CompletedAt { get; set; }
        public string WelcomeMessage { get; set; }
    }
}
