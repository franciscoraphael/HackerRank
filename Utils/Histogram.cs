public class Histogram<T>
{
    public Histogram(IEnumerable<T> enumerable)
    {
		LoadCollection(enumerable);
    }

    public IReadOnlyDictionary<T, int> Frequences { get; private set; }
	
	private void LoadCollection(IEnumerable<T> enumerable)
	{
		Dictionary<T,int> frequences = new Dictionary<T,int>();
		
		if (enumerable != null)
		{
			foreach (T item in enumerable)
			{
				if (frequences.ContainsKey(item))
				{
					frequences[item]++;
				}
				else
				{
					frequences.Add(item, 1);
				}
			}
		}
		
		this.Frequences = new ReadOnlyDictionary<T,int>(frequences);
	}
}
