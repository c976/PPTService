namespace Cvte.Net.Http
{
	public abstract class DynamicApiRequestAttribute : RequestAttribute
	{
		protected DynamicApiRequestAttribute()
			: this(RequestMethods.Default)
		{
		}

		protected DynamicApiRequestAttribute(RequestMethods requestMethod)
		{
			RequestMethod = requestMethod;
		}
	}
}
