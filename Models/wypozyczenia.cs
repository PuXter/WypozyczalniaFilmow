using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotationsExtensions;
using Microsoft.AspNetCore.Identity;

namespace WypozyczalniaFilmow.Models;

public class wypozyczenia
{
    [Key][DisplayName("Nr Wypożyczenia")]
    public int nrWypozyczenia { get; set; }
    [DisplayName("Data Wypożyczenia")]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode=true)]
    [DataType(DataType.Date)]
    public DateTime dataWypozyczenia { get; set; }
    [DisplayName("Ważność")]
    public int Waznosc { get; set; }
    [DisplayName("Aktywna")]
    public bool aktywna { get; set; }
    
    public filmy film { get; set; }
    
    [ForeignKey("filmy")][DisplayName("Tytuł filmu")]
    public string tytulFilmu { get; set; }
    
    [DisplayName("Id konta")]
    public virtual IdentityUser IdentityUser { get; set; }

    [DisplayName("Id konta")]
    public string IdentityUserId { get; set; }
}