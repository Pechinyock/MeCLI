namespace Me;

internal interface IDeserializeable<TModel>
{
    TModel Deserialze(ModelRepresentation format, string text);
}
