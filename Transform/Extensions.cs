using System.Collections.Generic;
using System.Text;

namespace Transform
{
	public static class Extensions
	{
		public static T[] ToArray<T>(this IReadOnlyList<T> list)
		{
			var result = new T[list.Count];
			for (var i = 0; i < result.Length; i++) result[i] = list[i];
			return result;
		}

		public static ILabel CreateStringLabel(this IEmitter emit, string value)
		{
			if (!emit.TryGetLabelByKey(value, out ILabel label)) label = emit.CreateLabel(null, value);

			emit.QueueAction(() =>
			{
				var buffer = Encoding.UTF8.GetBytes(value);
				emit.BeginData(label);
				for (var i = 0; i < buffer.Length; i++) emit.Push(buffer[i]);
				emit.Push((byte)0);
				emit.EndData();
			});

			return label;
		}
	}
}
