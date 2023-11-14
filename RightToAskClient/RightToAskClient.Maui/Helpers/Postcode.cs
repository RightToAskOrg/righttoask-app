using RightToAskClient.Maui.Models;

namespace RightToAskClient.Maui.Helpers
{
    public class Postcode
    {
        public static bool IsValid(ParliamentData.StateEnum selectedStateEnum, int postcode)
        {
            switch (selectedStateEnum)
            {
                case ParliamentData.StateEnum.ACT:
                    return (postcode >= 2600 && postcode <= 2618) || (postcode >= 2900 && postcode <= 2920);
                case ParliamentData.StateEnum.NSW:
                    return ((postcode >= 2000 && postcode <= 2599)
                            || (postcode >= 2619 && postcode <= 2898)
                            || (postcode >= 2921 && postcode <= 2999));
                case ParliamentData.StateEnum.NT:
                    return postcode >= 0800 && postcode <= 0899;
                case ParliamentData.StateEnum.QLD:
                    return postcode >= 4000 && postcode <= 4999;
                case ParliamentData.StateEnum.SA:
                    return postcode >= 5000 && postcode <= 5799;
                case ParliamentData.StateEnum.TAS:
                    return postcode >= 7000 && postcode <= 7799;
                case ParliamentData.StateEnum.VIC:
                    return postcode >= 3000 && postcode <= 3999;
                case ParliamentData.StateEnum.WA:
                    return postcode >= 6000 && postcode <= 6797;
                default:
                    return false;
            }
        }

    }
}