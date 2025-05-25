using Commons.Models;
using Microsoft.Extensions.Localization;
using Microsoft.VisualBasic;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;
using Presentation.MAUI.Resources;

namespace Presentation.MAUI.Services
{
    public class AlertService : IAlertService
    {
        private Task DisplayAlertAsync(string title, string message, string validationBtn = "OK")
        {
            return Shell.Current.DisplayAlert(title, message, validationBtn);
        }

        private Task<bool> DisplayConfirmationAsync(string title, string message, string accept, string cancel)
           => Shell.Current.DisplayAlert(title, message, accept, cancel);

        public async Task HandleResultAndResetAsync(Result result, BaseVM baseVM, bool resetWhenResultIsSuccess = true)
        {
            await ShowAsync(result);
            if (result != null && result.IsSuccess == resetWhenResultIsSuccess)
            {
                baseVM.Reset();
            }
        }

        public async Task ShowAsync(MessageType messageType, string? message)
        {
            if (message != null)
            {
                await DisplayAlertAsync(messageType.ToString(), message);
            }
        }

        public async Task ShowAsync(MessageType messageType, IEnumerable<string?>? messages)
        {
            if (messages != null && messages.Count() > 0)
            {
                await DisplayAlertAsync(messageType.ToString(), string.Join("\n", messages));
            }
        }

        public async Task ShowAsync(Result result)
        {
            if (result != null)
            {
                var messageType = result.IsSuccess ? MessageType.Success : MessageType.Error;
                var message = result.Message ?? string.Empty;
                await DisplayAlertAsync(messageType.ToString(), message);
            }
        }

        public Task<bool> ConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No", params object?[]? args)
        {
            var safeArgs = (args ?? Array.Empty<object?>())
               .Select(a => a ?? string.Empty)
               .ToArray();
            if (safeArgs.Length > 0)
            {

                message = string.Format(message, safeArgs);
            }

            return DisplayConfirmationAsync(title, message, accept, cancel);
        }
    }
}