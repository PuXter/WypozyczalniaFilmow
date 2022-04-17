using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotationsExtensions;

namespace WypozyczalniaFilmow.Models;

public enum Status
{
    Rozpatrywana,
    Pozytywna,
    Negatywna
}

public class reklamacje
{
    [Key][DisplayName("Nr Reklamacji")]
    public int nrReklamacji { get; set; }
    [DisplayName("Treść")]
    public string? tresc { get; set; }
    [DisplayName("Data reklamacji")]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode=true)]
    [DataType(DataType.Date)]
    public DateTime data { get; set; }
    [DisplayName("Status")]
    public Status status { get; set; }
    
    public wypozyczenia wypozyczenie { get; set; }
    
    [ForeignKey("wypozyczenia")][DisplayName("Nr wypożyczenia")]
    public int? nrWypozyczenia { get; set; }
}