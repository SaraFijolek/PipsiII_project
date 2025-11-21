using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Lines;

namespace Schematics.API.Service
{
    public class LineService : ILineService
    {
        private readonly ILineRepository _repository;
        private readonly ILineCategoryRepository _lineCategoryRepository;

        public LineService(ILineRepository repository, ILineCategoryRepository lineCategoryRepository)
        {
            _repository = repository;
            _lineCategoryRepository = lineCategoryRepository;
        }

        public async Task AddLineAsync(LineDto model)
        {
            var category = await _lineCategoryRepository.GetByIdAsync(model.LineCategoryId);
            if (category == null)
                throw new KeyNotFoundException("Kategoria linii nie istnieje.");

            var newLine = new LineDb
            {
                SchemaId = model.SchemaId,
                LineCategoryId = model.LineCategoryId,
                Name = model.Name,
                LineNumber = model.LineNumber,
                Color = model.Color,
                IsCircular = model.IsCircular
            };

            await _repository.AddAsync(newLine);
        }

        public async Task<IList<LineDto>> GetAllLinesAsync()
        {
            var lines = await _repository.GetAllAsync();

            return lines.Select(line => new LineDto
            {
                Id = line.Id,
                SchemaId = line.SchemaId,
                LineCategoryId = line.LineCategoryId,
                Name = line.Name,
                LineNumber = line.LineNumber,
                Color = line.Color,
                IsCircular = line.IsCircular
            }).ToList();
        }

        public async Task<IList<LineDto>> GetLinesBySchemaIdAsync(int schemaId)
        {
            var allLines = await _repository.GetAllAsync();
            var lines = allLines.Where(l => l.SchemaId == schemaId).ToList();

            return lines.Select(line => new LineDto
            {
                Id = line.Id,
                SchemaId = line.SchemaId,
                LineCategoryId = line.LineCategoryId,
                Name = line.Name,
                LineNumber = line.LineNumber,
                Color = line.Color,
                IsCircular = line.IsCircular
            }).ToList();
        }
    }
}
