package bc.assets;

public abstract class ContentProcessor <InputType, OutputType>
{
	public abstract OutputType process(InputType input, ContentProcessorContext context);
}
