using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WordMaster.Generation.DTOs;
using WordMaster.Generation.Interfaces;

namespace WordMaster.Generation.Stages
{
    internal abstract class StageBase : IStage
    {
        protected abstract string InitialPrompt { get; }
        protected abstract string RetryPrompt { get; }
        protected abstract string ToNextStagePrompt { get; }
        protected abstract string StageName { get; }
        protected abstract string ResponseName { get; }
        protected abstract string ResonseSchema { get; }
        protected string? LastResponse { get; set; }


        public abstract Task ExecuteAsync(IContext context, ISessionController control);

        protected void EnsureSuccessResponse()
        {
            if (string.IsNullOrWhiteSpace(LastResponse))
            {
                throw new Exception("Agent didn't provide a response.");
            }

            using JsonDocument doc = JsonDocument.Parse(LastResponse);
            var root = doc.RootElement;

            if (!root.TryGetProperty("success", out var success) || !success.GetBoolean())
            {
                var message = root.TryGetProperty("error_message", out var error)
                    ? error.GetString()
                    : "Unknown error.";

                throw new Exception($"Agent couldn't complete request: {message}");
            }
        }


        protected async Task Complete(IContext context)
        {
            context.History.Add(new UserChatMessage(InitialPrompt));

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: ResponseName,
                    jsonSchema: BinaryData.FromString(ResonseSchema),
                    jsonSchemaIsStrict: true)
            };

            ChatCompletion completion = await context.Chat.CompleteChatAsync(context.History, options);

            context.History.Add(new AssistantChatMessage(completion));

            LastResponse = completion.Content[0].Text;
        }

        protected virtual Task OnControllerActionAsync(SessionControllerAction action, IContext context)
        {
            var prompt = action switch
            {
                SessionControllerAction.Repeat => RetryPrompt,
                SessionControllerAction.Continue => ToNextStagePrompt,
                SessionControllerAction.Cancel => null,
                _ => throw new NotSupportedException($"Action {action} is not supported.")
            };

            context.History.Add(new UserChatMessage(prompt));

            return Task.CompletedTask;
        }

        Task IStage.OnControllerActionAsync(SessionControllerAction action, IContext context)
        {
            return OnControllerActionAsync(action, context);
        }
    }
}
