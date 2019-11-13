using System;

namespace PodcastCore.AudioBlobInbox.Services.Exceptions
{
    public class ContainerNotSupportedException : Exception
    {
        public ContainerNotSupportedException(string containerFormatName) : base(MakeErrorMessage(containerFormatName))
        {
            ContainerFormatName = containerFormatName;
        }

        public string ContainerFormatName { get; }

        private static string MakeErrorMessage(string containerFormatName)
        {
            return $"Container format \"{containerFormatName}\" is not supported";
        }
    }
}