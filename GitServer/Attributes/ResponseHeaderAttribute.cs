using Microsoft.AspNetCore.Mvc.Filters;

namespace GitServer.Attributes
{
	public class ResponseHeaderAttribute : ActionFilterAttribute
    {
		public string Key { get; protected set; }
		public string Value { get; protected set; }

		public ResponseHeaderAttribute(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			context.HttpContext.Response.Headers.Add(Key, Value);
			base.OnActionExecuting(context);
		}
	}
}
