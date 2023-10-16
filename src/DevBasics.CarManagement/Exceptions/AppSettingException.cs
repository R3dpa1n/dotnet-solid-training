using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DevBasics.CarManagement.Exceptions
{
	[Serializable]
	public class AppSettingException : Exception
	{
		/// <inheritdoc />
		public AppSettingException()
		{
		}

		/// <inheritdoc />
		public AppSettingException(string message)
			: base(message)
		{
		}

		/// <inheritdoc />
		public AppSettingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <inheritdoc />
		protected AppSettingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
