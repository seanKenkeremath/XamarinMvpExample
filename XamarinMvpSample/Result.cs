using System;
namespace XamarinMvpSample
{
	public class Result<T>
	{
		public readonly T Data;
		public readonly Error Error;

		public Result(T data)
		{
			Data = data;
			Error = Error.None;
		}
		public Result(Error error)
		{
			Data = default(T);
			Error = error;
		}
	}
}
