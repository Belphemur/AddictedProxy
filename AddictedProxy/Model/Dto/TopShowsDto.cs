using System.ComponentModel.DataAnnotations;

namespace AddictedProxy.Model.Dto;

public record TopShowDto([Required] ShowDto Show, [Required] long Popularity);