namespace OpenDCP.Framework.Hosting.Exceptions
{
    internal static class ErrorMessages
    {
        //StringExtensions
        public const string IntervalWrongAbbreviaton = "Wrong interval abbreviation";
        public const string IntervalNotNumber = "Interval is not recognize as valid integer";

        //Message Bus
        public const string UnkownMessageBusType = "This message bus type is not supported";
        public const string InvalidMessageBusConfig = "Invalid Message bus configuration";

        //Token
        public const string TokenDetailsNotDefined = "Missing token details";

        //RangeChecker
        public const string InvalidConditionType = "Invalid condition type";
        //Attribute Filters
        public const string SessiontTokenExpriedOrNotValid = "Session token expired or not valid";
        public const string PersistedGrantsTypeRequired = "Persisted grants type required";
        public const string PersistedGrantsTypeNotSupportedExtension = "Persisted grants type not supported format";
        public const string UserClientCredentialsCanBeUsedOnlyFromAPIGateway = "User client credentials can be used only from API Gateway";

        //Datype
        public const string DatatypeMismatch = "Datatype not matching";
    }
}
