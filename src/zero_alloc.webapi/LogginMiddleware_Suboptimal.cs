using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace zero_alloc.webapi
{
	using LogRequestAction = Action<ILogger, string, HostString, PathString, QueryString, string, string, Exception>;
	using LogResponseAction = Action<ILogger, int, string, Exception>;

	public class LoggingMiddlewareSuboptimal
	{
		private readonly ILogger _logger;
		private readonly RequestDelegate _next;
		
		private const int ProductionBodyLengthLimit = 8 * 1024;

		public LoggingMiddlewareSuboptimal(RequestDelegate next, ILogger<LoggingMiddlewareSuboptimal> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			context.Request.EnableBuffering();

			using (var buffer = new MemoryStream(capacity: 8192))
			{
				var original = context.Response.Body;
				context.Response.Body = buffer;

				await LogRequest(context.Request);
				await _next(context);
				await LogResponse(context.Response);

				await CopyFromBuffer(context.Response, original);
			}
		}

		private async Task LogRequest(HttpRequest request)
		{
			var requestBodyText = await ReadAsText(request.Body);
			//var headers = ReadHeaders(request.Headers);
			_logger.LogInformation(requestBodyText);
		}

		private async Task LogResponse(HttpResponse response)
		{
			var responseBodyText = await ReadAsText(response.Body);

			_logger.LogInformation(responseBodyText);
		}

		private async Task<string> ReadAsText(Stream stream)
		{
			var encoding = Encoding.UTF8;
			var length = encoding.GetMaxByteCount(ProductionBodyLengthLimit);
			var buffer = default(byte[]);

			try
			{
				buffer = ArrayPool<byte>.Shared.Rent(length);
				stream.Seek(0, SeekOrigin.Begin);
				var actual = await stream.ReadAsync(buffer, 0, length);
				stream.Seek(0, SeekOrigin.Begin);

				return encoding.GetString(buffer, 0, actual);
			}
			finally
			{
				if (buffer != null)
				{
					ArrayPool<byte>.Shared.Return(buffer);
				}
			}
		}

		private static string ReadHeaders(IHeaderDictionary headers)
		{
			var result = new StringBuilder(capacity: 256);

			foreach (var header in headers)
			{
				if (header.Value.Count == 0)
				{
					continue;
				}

				result.AppendFormat("Name: {0}, Values: ", header.Key);

				foreach (var value in header.Value)
				{
					result.AppendFormat("{0}, ", value);
				}

				result.AppendFormat(Environment.NewLine);
			}

			return result.ToString();
		}

		private static async Task CopyFromBuffer(HttpResponse response, Stream original)
		{
			response.Body.Seek(0, SeekOrigin.Begin);
			await response.Body.CopyToAsync(original);
			response.Body = original;
		}
	}
}