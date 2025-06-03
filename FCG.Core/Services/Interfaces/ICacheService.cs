using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Core.Services.Interfaces;

public interface ICacheService
{
    object Get(string key);
    void Set(string key, object content);
    void Remove(string key);
}