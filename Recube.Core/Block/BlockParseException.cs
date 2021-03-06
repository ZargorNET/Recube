using System;

namespace Recube.Core.Block
{
    public class BlockParseException : Exception
    {
        public BlockParseException()
        {
        }

        public BlockParseException(string message) : base(message)
        {
        }

        public BlockParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}