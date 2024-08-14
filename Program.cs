using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace gRPCBasedDotnetFramework
{
	/// <summary>
	/// 자동 생성된 GreetGrpc.cs에서 클래스 호출
	/// </summary>
	public class GreeterService : Greeter.GreeterBase
	{
		/// <summary>
		/// 파라미터는 자동 생성된 Greet.cs에서 정의 + Grpc.Core에서 이미 정의된 타입
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Task<HelloReply> SayHello (HelloRequest request, ServerCallContext context)
		{
			return Task.FromResult(new HelloReply
			{
				Message = $"Hello, {request.Name}!"
			});
		}

		/// <summary>
		/// 파라미터는 자동 생성된 Greet.cs에서 정의 + Grpc.Core에서 이미 정의된 타입
		/// </summary>
		/// <param name="request"></param>
		/// <param name="responseStream"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override async Task SayHelloStream (HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
		{
			// 반복해서 처리
			for (int i = 0; i < 5; i++)
			{
				// 서버 측에서 응답 스트림에 접근해서 비동기로 HelloReply를 작성해준다
				await responseStream.WriteAsync(new HelloReply
				{
					Message = $"Hello, {request.Name}! Message {i + 1}"
				});
				await Task.Delay(1000); // 1초 대기
			}
		}

		/// <summary>
		/// 파라미터는 자동 생성된 Greet.cs에서 정의 + Grpc.Core에서 이미 정의된 타입
		/// </summary>
		/// <param name="requestStream"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override async Task<HelloReply> SayHelloClientStream (IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
		{
			// 스트링 빌더를 만들어준다
			var sb = new StringBuilder();

			// 클라이언트 측 스트리밍의 요청을 보고 MoveNext()를 할 수 없을 때까지, 그러니까 모든 요청을 처리할 때까지
			while (await requestStream.MoveNext())
			{
				// sb에 내용을 작성한다
				sb.AppendLine($"Hello, {requestStream.Current.Name}!");
			}

			// HelloReply를 반환하되 메시지는 내용 들어간 sb로 처리
			return new HelloReply { Message = sb.ToString() };
		}
	}

	internal class Program
	{
		static void Main (string[] args)
		{
			const int Port = 50051;

			// 이 서버는 Grpc.Core 기반으로 생성됨
			var server = new Server
			{
				Services = { Greeter.BindService(new GreeterService()) },
				// 기본적으로 ServerCredentials.Insecure 옵션에 의해 http://와 같이 실행됨
				Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
			};

			#region 만약 HTTPS 보안 옵션으로 실행하고 싶다면
			/**
			 * var securedServer = new Server
			 * {
			 *		Services = { Greeter.BindService(new GreeterService()) },
			 *		Ports =
			 *		{
			 *			new ServerPort("localhost", 50001, new SslServerCredentials(new List<KeyCertificatePair>
			 *			{
			 *				new KeyCertificatePair(File.ReadAllText("server.crt"), File.ReadAllText("server.key"))
			 *			}))
			 *		}
			 *	};
			 *	
			 *	Ssl 서버 인증에 대해 적는다, 인수로는 키와 인증 페어가 들어가고 서버 인증서와 서버 키가 들어갈 수 있다.
			 */
			#endregion

			// 서버 구동
			server.Start();

			// 포트 설명
			Console.WriteLine($"Greeter server listening on port {Port}");

			// 아무 키나 누르면 서버가 꺼진다, while loop와 Console.ReadLine 등을 통해 피해갈 수 있음
			Console.WriteLine("Press any key to stop the server...");

			// 키 누르기
			Console.ReadKey();

			// 더 이상 서비스되는 콜이 없을 때 서버를 끄도록 하며 사용한 리소스를 정리.
			// 끄는 프로시저가 완료됐을 때 반환된 태스크를 마친다.
			// '프로세스를 종료하기 전에 이전에 생성된 모든 서버를 종료하는 것이 좋습니다.'
			server.ShutdownAsync().Wait();
		}
	}
}

