using Commons;
using Commons.ErrorsHandlings;

using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;

namespace Presentation.MAUI.Services
{
    public class AlertService : IAlertService
    {
        private Task DisplayAlertAsync(string title, string message, string validationBtn = "OK")
        {
            if (Shell.Current != null)
                return Shell.Current.DisplayAlert(title, message, validationBtn);

            return Application.Current?.MainPage?.DisplayAlert(title, message, validationBtn)
                ?? Task.CompletedTask;
        }

        private Task<bool> DisplayConfirmationAsync(string title, string message, string accept, string cancel)
           => Shell.Current.DisplayAlert(title, message, accept, cancel);

        public async Task HandleResultAndResetAsync(IServiceResult result, BaseVM baseVM, bool resetWhenResultIsSuccess = true, bool showSuccess = true)
        {
            await ShowAsync(result, showSuccess);
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

        public async Task ShowAsync(IServiceResult result, bool showSuccess = false)
        {
            var msg = Presentation.MAUI.Resources.Localization.DialogsStrings.CommonsNo;

            bool shouldShow = !result.IsSuccess || (result.IsSuccess && showSuccess);
            if (shouldShow)
            {
                await ShowAsync(result.MessageType, result.Message);
            }
        }

        public async Task ShowAsync(MessageType messageType,string message, params object?[]? args)
        {
            var safeArgs = (args ?? Array.Empty<object?>())
                .Select(a => a ?? string.Empty)
                .ToArray();
            if (safeArgs.Length > 0)
            {
                message = string.Format(message, safeArgs);
            }
            await ShowAsync(messageType, message);
        }
    }
}