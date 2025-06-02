using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Models;

namespace WordMaster.Data.DTOs.Mappers
{
    internal static class WordMapper
    {
        public static WordDTO ToDTO(this Word entity)
        {
            return new WordDTO
            {
                Id = entity.Id,
                Text = entity.Text,
                Translation = entity.Translation,
                Definition = entity.Definition
            };
        }

        public static Word ToEntity(this WordDTO dto)
        {
            return new Word
            {
                Id = dto.Id,
                Text = dto.Text,
                Translation = dto.Translation,
                Definition = dto.Definition ?? string.Empty
            };
        }
    }
}
