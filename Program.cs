using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace gRPCBasedDotnetFramework
{
	/// <summary>
	/// 자동 생성된 GreetGrpc.cs에서 클래스 호출
	/// </summary>
	public class GreeterService : Greeter.GreeterBase
	{
		/// <summary>
		/// 파라미터는 자동 생성된 Greet.cs에서 정의한 형태
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

