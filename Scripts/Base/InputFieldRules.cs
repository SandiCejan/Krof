namespace KrofEngine
{
    public class InputFieldRules
    {
        public bool Numbers = true, LowerCharacters = true, UpperCharacters = true, Spaces = true;

        public InputFieldRules(bool numbers, bool lowerCharacters, bool upperCharacters, bool spaces)
        {
            Numbers = numbers;
            LowerCharacters = lowerCharacters;
            UpperCharacters = upperCharacters;
            Spaces = spaces;
        }
        public InputFieldRules()
        {
        }
    }
}
