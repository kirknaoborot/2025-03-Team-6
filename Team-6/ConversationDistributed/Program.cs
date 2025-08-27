using ConversationDistributed;
using ConversationDistributed.Consumers;
using ConversationDistributed.Services;
using MassTransit;
using OrchestratService.Application;


var builder = Host.CreateApplicationBuilder(args);

// ������ ��������� �������������
builder.Services.AddSingleton<IUserStateService, UserStateService>();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserLoggedInConsumer>();
    x.AddConsumer<CreateConversationEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // �������: ��� �����
        cfg.ReceiveEndpoint("user-login-event-queue", e =>
        {
            e.ConfigureConsumer<UserLoggedInConsumer>(context);
        });

        // �������: ����� ���������
        cfg.ReceiveEndpoint("create-conversation-event-queue", e =>
        {
            e.ConfigureConsumer<CreateConversationEventConsumer>(context);
        });

        // ������� ��� ��������� ����������� 
        cfg.ReceiveEndpoint("assignment-proposals-queue", e =>
        {
            // ����� �������� consumer
        });
    });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();