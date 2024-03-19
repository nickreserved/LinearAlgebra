namespace MGroup.LinearAlgebra.Distributed
{
	using MGroup.LinearAlgebra.Distributed.LinearAlgebraExtensions;

	public interface IDistributedIndexer
    {
        bool IsCompatibleWith(IDistributedIndexer other);

		public static readonly NonMatchingFormatException IncompatibleIndexersException
			= new NonMatchingFormatException("The provided tensor or transformation has a different format than this indexer. " +
					"Their entries correspond to different dofs or they are distributed differently across compute nodes.");
	}
}
