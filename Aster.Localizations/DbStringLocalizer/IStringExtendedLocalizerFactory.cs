using Microsoft.Extensions.Localization;

namespace Aster.Localizations.DbStringLocalizer
{
    public interface IStringExtendedLocalizerFactory : IStringLocalizerFactory
    {
        //Task ResetCache();

        //Task ResetCache(Type resourceSource);

        //Task ResetCache(string resourceKey);


        //Task CreateOrUpdateLocalization(params Localization[] localizations);
    }
}
