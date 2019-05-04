public static class StringExtension
{
    public static Anagram AsAnagram(this string s)
    {
        return new Anagram(s ?? string.Empty);
    }
}

public class Anagram
{
    private const int alphabetCharsCount = 26;
    private List<int> charsCount = new List<int>(alphabetCharsCount) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public IReadOnlyList<int> Chars
    {
        get
        {
            return charsCount.AsReadOnly();
        }
    }

    public Anagram(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            charsCount[PositionInAlphabet(s[i])]++;
        }
    }

    public Anagram(IEnumerable<int> chars)
    {
        if (chars == null || chars.Count() != alphabetCharsCount) throw new ArgumentException("Parameter 'chars' should have exactly 26 positions.");

        if (chars.Any(x => x < 0)) throw new ArgumentException("All chars values should be not negative.");

        charsCount = new List<int>(chars);
    }

    public int Length(){
        return this.charsCount.Sum();
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Anagram);
    }

    public bool Equals(Anagram other)
    {
        IReadOnlyList<int> othersCharsCount = other.Chars;

        for (int i = 0; i < alphabetCharsCount; i++)
        {
            if (this.charsCount[i] != othersCharsCount[i])
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hashCode = 0;

        for (int i = 0; i < alphabetCharsCount; i++)
        {
            hashCode += (int)Math.Pow(i+1, 4) * this.charsCount[i];
        }

        return hashCode;
    }

    public Anagram Difference(string str)
    {
        return this.Difference(new Anagram(str));
    }

    public Anagram Difference(Anagram other)
    {
        List<int> difference = new List<int>(other.Chars);

        for (int i = 0; i < alphabetCharsCount; i++)
        {
            difference[i] = Math.Abs(this.charsCount[i] - difference[i]);
        }

        return new Anagram(difference);
    }

    private int PositionInAlphabet(char c)
    {
        return c - 'a';
    }
}
