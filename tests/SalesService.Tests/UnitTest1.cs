namespace SalesService.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_PublishesEvent()
    {
        var publishEndpoint = Substitute.For<IPublishEndpoint>();
        var service = new OrderService(publishEndpoint);

        await service.CreateOrder(1, 2);

        await publishEndpoint.Received(1).Publish(Arg.Any<OrderCreatedEvent>());
    }
}
