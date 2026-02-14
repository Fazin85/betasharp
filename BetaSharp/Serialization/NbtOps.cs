using BetaSharp.NBT;
using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace BetaSharp.Serialization;

public sealed class NbtOps : IDynamicOps<NBTBase>
{
    public NBTBase Empty() => new NBTTagCompound();

    public NBTBase CreateNumeric(decimal number)
    {
        // Prefer the smallest type that can hold it
        if (number == Math.Floor(number))
        {
            return number switch
            {
                >= sbyte.MinValue and <= sbyte.MaxValue => new NBTTagByte((sbyte)number),
                >= short.MinValue and <= short.MaxValue => new NBTTagShort((short)number),
                >= int.MinValue and <= int.MaxValue => new NBTTagInt((int)number),
                _ => new NBTTagLong((long)number),
            };
        }

        // Check for float, otherwise default to double
        if ((float)number == (double)number)
            return new NBTTagFloat((float)number);

        return new NBTTagDouble((double)number);
    }

    public NBTBase CreateString(string value) => new NBTTagString(value);

    public NBTBase CreateBool(bool value)
    {
        return new NBTTagByte((sbyte)(value ? 1 : 0)); // we just encode as an 8-bit integer
    }

    public DataResult<bool> GetBool(NBTBase input)
    {
        if (input.GetTagType() != 1 || input is not NBTTagByte byteTag)
            return DataResult<bool>.Fail("Input is not a byte tag, cannot decode as boolean value");

        var value = (byte)byteTag.Value;
        if (value > 1) // Only accept 0 and 1 since we decode that way
            return DataResult<bool>.Fail(
                $"Cannot decode byte value above 1 to boolean (Found: {value})"
            );

        return DataResult<bool>.Success(value == 1 ? true : false);
    }

    public DataResult<decimal> GetNumber(NBTBase input)
    {
        switch (input)
        {
            case NBTTagByte byteTag:
                return Success(byteTag.Value);

            case NBTTagShort shortTag:
                return Success(shortTag.Value);

            case NBTTagInt intTag:
                return Success(intTag.Value);

            case NBTTagLong longTag:
                return Success(longTag.Value);

            case NBTTagFloat floatTag:
                return Success((decimal)floatTag.Value);

            case NBTTagDouble doubleTag:
                return Success((decimal)doubleTag.Value);

            default:
                return DataResult<decimal>.Fail("Input is not a numeric NBT tag");
        }

        DataResult<decimal> Success(decimal value) => DataResult<decimal>.Success(value);
    }

    public DataResult<string> GetString(NBTBase input)
    {
        if (input.GetTagType() != 8 || input is not NBTTagString stringTag)
            return DataResult<string>.Fail("Input is not a string NBT tag", string.Empty);

        return DataResult<string>.Success(stringTag.Value);
    }

    public DataResult<NBTBase> GetValue(NBTBase input, string name)
    {
        switch (input)
        {
            case NBTTagCompound compoundTag:
                return compoundTag.HasKey(name)
                    ? DataResult<NBTBase>.Success(compoundTag.GetCompoundTag(name))
                    : DataResult<NBTBase>.Fail($"Compound NBT tag does not contain key '{name}'");

            case NBTTagList listTag:
                if (int.TryParse(name, out var index) && index >= 0 && index < listTag.TagCount())
                    return DataResult<NBTBase>.Success(listTag.TagAt(index));
                return DataResult<NBTBase>.Fail($"List index '{name}' is invalid or out of bounds");

            default:
                return DataResult<NBTBase>.Fail(
                    $"Cannot get value from NBT tag type {NBTBase.GetTagName(input.GetTagType())}"
                );
        }
    }

    public NBTBase CreateList(IEnumerable<NBTBase> elements)
    {
        if (elements.All(e => e is NBTTagByte byteTag))
        {
            var bytes = elements.Cast<NBTTagByte>().Select(b => (byte)b.Value).ToArray();
            return new NBTTagByteArray(bytes);
        }

        var listTag = new NBTTagList();
        foreach (var element in elements)
            listTag.SetTag(element);

        return listTag;
    }

    public DataResult<IEnumerable<NBTBase>> ReadAsStream(NBTBase input)
    {
        switch (input)
        {
            case NBTTagList listTag:
                return DataResult<IEnumerable<NBTBase>>.Success(listTag.AsEnumerable());

            case NBTTagByteArray byteArrayTag:
                return DataResult<IEnumerable<NBTBase>>.Success(
                    byteArrayTag.Values.Select(b => new NBTTagByte((sbyte)b))
                );

            default:
                return DataResult<IEnumerable<NBTBase>>.Fail(
                    "Input is not a list or byte array tag."
                );
        }
    }

    public NBTBase CreateMap(IEnumerable<KeyValuePair<NBTBase, NBTBase>> map)
    {
        var compoundTag = new NBTTagCompound();
        foreach (var kvp in map)
        {
            if (kvp.Key is not NBTTagString stringTag)
                throw new InvalidOperationException("NBT map keys must be strings."); // fail loudly to prevent bugs from slipping through
            compoundTag.SetTag(stringTag.Value, kvp.Value);
        }

        return compoundTag;
    }

    public DataResult<IEnumerable<KeyValuePair<NBTBase, NBTBase>>> ReadAsMap(NBTBase input)
    {
        if (input is not NBTTagCompound compoundTag)
            return DataResult<IEnumerable<KeyValuePair<NBTBase, NBTBase>>>.Fail(
                "Input is not a compound tag."
            );

        return DataResult<IEnumerable<KeyValuePair<NBTBase, NBTBase>>>.Success(
            compoundTag.Values.Select(val => new KeyValuePair<NBTBase, NBTBase>(
                new NBTTagString(val.Key),
                val
            ))
        );
    }

    public NBTBase RemoveFromInput(NBTBase input, NBTBase value)
    {
        throw new NotImplementedException();
    }

    public NBTBase AppendToPrefix(NBTBase prefix, NBTBase value)
    {
        switch (prefix)
        {
            case NBTTagByteArray byteArrayTag:
                return AppendToByteArray(byteArrayTag, value);

            case NBTTagList listTag:
                return AppendToList(listTag, value);

            case NBTTagCompound compountTag:
                if (value is not NBTTagCompound otherCompound)
                    throw new InvalidOperationException(
                        "Cannot append non-compound tag to compound tag."
                    );

                foreach (var tag in otherCompound.Values)
                    compountTag.SetTag(tag.Key, tag);

                return compountTag;

            default:
                throw new InvalidOperationException(
                    $"Cannot append to NBT tag type {NBTBase.GetTagName(prefix.GetTagType())}"
                );
        }
    }

    public NBTBase Merge(NBTBase key, NBTBase value)
    {
        if (key is not NBTTagString stringTag)
            throw new InvalidOperationException("NBT map keys must be strings.");

        var obj = new NBTTagCompound();
        obj.SetTag(stringTag.Value, value);

        return obj;
    }

    public NBTBase MergeAndAppend(NBTBase map, NBTBase key, NBTBase value)
    {
        return AppendToPrefix(map, Merge(key, value));
    }

    private NBTBase AppendToList(NBTTagList listTag, NBTBase value)
    {
        void AppendSingle(NBTBase element)
        {
            if (listTag.TagCount() > 0 && listTag.TagAt(0).GetTagType() != element.GetTagType())
                throw new InvalidOperationException(
                    $"Cannot append element that is not {NBTBase.GetTagName(element.GetTagType())} - NBT list tag must remain homogeneous"
                );

            listTag.SetTag(element);
        }

        if (value is NBTTagList otherlistTag)
        {
            foreach (var tag in otherlistTag)
                AppendSingle(tag);
        }
        else
        {
            AppendSingle(value);
        }

        return listTag;
    }

    private NBTBase AppendToByteArray(NBTTagByteArray array, NBTBase value)
    {
        var existing = array.Values;

        switch (value)
        {
            case NBTTagByte singleByte:
                array.Values = existing.Append((byte)singleByte.Value).ToArray();
                break;

            case NBTTagByteArray other:
                array.Values = existing.Concat(other.Values).ToArray();
                break;

            case NBTTagList listTag when listTag.All(t => t is NBTTagByte):
                var bytes = listTag.Cast<NBTTagByte>().Select(b => (byte)b.Value);
                array.Values = existing.Concat(bytes).ToArray();
                break;

            default:
                throw new InvalidOperationException(
                    "Cannot append non-byte or non-byte array value to byte array."
                );
        }

        return array;
    }
}
