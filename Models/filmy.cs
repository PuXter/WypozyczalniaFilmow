using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotationsExtensions;

namespace WypozyczalniaFilmow.Models;

public class filmy
{
    [Key][DisplayName("Tytuł")]
    public string tytulFilmu { get; set; }
    [DisplayName("Reżyser")]
    public string? rezyser { get; set; }
    [Range(1895, 2022,ErrorMessage = "The field {0} must be greater than {1} and lower than {2}")][DisplayName("Rok produkcji")]
    public int? rokProdukcji { get; set; }
    [Min(0)][DisplayName("Cena")]
    public int cena { get; set; }
    [Range(0,10,ErrorMessage = "The field {0} must be greater than {1} and lower than {2}.")][DisplayName("Ocena")]
    public int? ocena { get; set; }
}