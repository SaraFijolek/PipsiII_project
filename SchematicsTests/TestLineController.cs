using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.DTOs.Lines;
using Schematics.API.Service;

namespace SchematicsTests
{
    [TestClass]
    public class TestLineController
    {
        private Mock<ILineService> _lineService;
        private LineController _lineController;

        [TestInitialize]
        public void Setup()
        {
            _lineService = new Mock<ILineService>();
            _lineController = new LineController(_lineService.Object);
        }

        [TestMethod]
        public async Task TestGetAllLines()
        {
            IList<LineDto> lines = new List<LineDto>();
            lines.Add(new LineDto { Id = 1 });
            lines.Add(new LineDto { Id = 2 });
            _lineService.Setup(service => service.GetAllLinesAsync()).ReturnsAsync(lines);

            ActionResult<IList<LineDto>> result = await _lineController.GetAllLines();

            result.Should().NotBeNull();
        }
    }
}
