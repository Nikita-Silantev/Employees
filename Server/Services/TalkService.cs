using Grpc.Contracts;
using Grpc.Core;

namespace Server.Services;

public class TalkService : Talk.TalkBase
{
    public override Task<WordResponse> Smalltalk(WordRequest request, ServerCallContext context)
    {
        return Task.FromResult(new WordResponse
        {
            Word = "Hello World from grpc server!"
        });
    }
}