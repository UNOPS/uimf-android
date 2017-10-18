namespace AndroidApp
{
	public class MenuMetadata
	{
		public MenuMetadata(string name, int orderIndex = 0)
		{
			this.Name = name;
			this.OrderIndex = orderIndex;
		}

		public string Name { get; set; }

		public int OrderIndex { get; set; }
	}
}