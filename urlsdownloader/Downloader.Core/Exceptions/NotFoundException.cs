namespace Downloader.Core.Exceptions
{
    using System;

    public class NotFoundException : Exception
    {
        public NotFoundException(string msg) : base(msg)
        {
        }
    }
}
