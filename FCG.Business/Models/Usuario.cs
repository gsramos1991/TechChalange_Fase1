using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Business.Models;
public class Usuario : IdentityUser
{
    public string Nome { get; set; }    
}
