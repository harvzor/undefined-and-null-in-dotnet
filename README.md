## PATCH

### `PUT /api/customers/broken/{id}`

Assuming the following document:

```json
{
"id": 1,
"name": "Harvey",
"gender": "male",
"deletedDate": "2023-06-14T18:22:55.1646444+02:00"
}
```

If I then make the following request:

```
PUT /api/customers/broken/1
{
  "name": "Harvzor"
}
```

The final document is:

```json
{
  "id": 1,
  "name": "Harvzor",
  "gender": null,
  "deletedDate": null
}
```

Note that I did not send `gender` or `deleted`, so my C# DTO was hydrated with the `default` values (`null` and `null`). It's not possible for me to know if the client explicitly set the property to `null`, or it happened when the property was instantiated.

A solution may be to put this in the code:

```csharp
[Required] // add this
public string? Gender { get; set; }
```

But now I can't explicitly set `gender` to be null (`"name": "Harvzor"`), as `[Required]` doesn't allow null values at all.

## Further reading

- https://github.com/dotnet/csharplang/discussions/385 people arguing about what to do
- https://github.com/DavidAmesPup/OptionalTypes/ solves the issue only for Newtonsoft
- https://hacklewayne.com/patch-friendly-types-null-confusion-undefined-envy-and-maybe-maybe explains the issue very well
- looks like Google has created Optional for Java: https://guava.dev/releases/19.0/api/docs/com/google/common/base/Optional.html
- https://github.com/ladeak/JsonMergePatch source generater solution
