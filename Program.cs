var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// /api/jarvis 호출 주소 생성
app.MapPost("/api/jarvis", (VoiceRequest request) =>
{
    Console.WriteLine($"[자비스 수신]: {request.Message}");

    string responseMessage = $"주인님, '{request.Message}'라고 말씀하셨군요. 자비스 서버 연동 성공입니다.";

    return Results.Ok(new { reply = responseMessage });
});

app.Run();

// 클래스(형식) 선언은 실행 문장(app.Run)보다 가장 아래쪽에 배치해야 합니다.
public class VoiceRequest
{
    public string Message { get; set; } = string.Empty;
}