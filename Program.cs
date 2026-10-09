using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 기본 헬스체크용 GET 엔드포인트
app.MapGet("/", () => "JARVIS Server is Running!");

// 자비스 음성 요청 처리 POST 엔드포인트
app.MapPost("/api/jarvis", async (JarvisRequest request) =>
{
    // 1. 환경 변수에서 OpenAI API Key 읽기
    string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    if (string.IsNullOrEmpty(apiKey))
    {
        return Results.Ok(new JarvisResponse("주인님, 서버에 OPENAI_API_KEY가 설정되지 않았습니다."));
    }

    if (string.IsNullOrWhiteSpace(request.Message))
    {
        return Results.Ok(new JarvisResponse("음성 입력이 전달되지 않았습니다. 다시 말씀해 주세요."));
    }

    try
    {
        // 2. OpenAI ChatClient 생성 (가성비 및 반응속도가 빠른 gpt-4o-mini 사용)
        ChatClient client = new(model: "gpt-4o-mini", apiKey: apiKey);

        // 3. 자비스 페르소나(System Prompt) 및 사용자 메시지 구성
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(
                "너는 영화 아이언맨에 나오는 지능적이고 유능한 인공지능 비서 '자비스(JARVIS)'이다. " +
                "사용자를 '주인님'이라 부르며, 친절하고 정중하게 한국어로 답변하라. " +
                "답변은 스마트폰 음성(TTS)으로 출력되므로 2~3문장 이내로 핵심만 간결하게 말하라."
            ),
            new UserChatMessage(request.Message)
        };

        // 4. LLM 답변 생성 요청
        ChatCompletion completion = await client.CompleteChatAsync(messages);
        string aiReply = completion.Content[0].Text;

        Console.WriteLine($"[수신]: {request.Message}");
        Console.WriteLine($"[응답]: {aiReply}");

        return Results.Ok(new JarvisResponse(aiReply));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[오류 발생]: {ex.Message}");
        return Results.Ok(new JarvisResponse($"오류가 발생했습니다: {ex.Message}"));
    }
});

app.Run();

// 요청/응답 DTO 정의
record JarvisRequest(string Message);
record JarvisResponse(string Reply);