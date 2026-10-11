using System.Linq.Expressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Tasko.Data;

public class EncryptedStringConverter : ValueConverter<string?, string?>
{
    public EncryptedStringConverter(IDataProtector protector) 
        : base(plain => plain == null ? null : protector.Protect(plain),
            cipher => cipher == null ? null : protector.Unprotect(cipher))
    {

    }
}
