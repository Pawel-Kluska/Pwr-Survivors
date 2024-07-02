using System.Text;
using System.Reflection;
using UnityEngine;
using TMPro;

public class UIStatsDisplay : MonoBehaviour
{
    public PlayerStats player;
    public bool updateInEditor = false;
    TextMeshProUGUI statNames, statValues;

    void OnEnable()
    {
        UpdateStatFields();
    }

    void OnDrawGizmosSelected()
    {
        if (updateInEditor) UpdateStatFields();
    }

    public void UpdateStatFields()
    {
        if (!player) return;

        if (!statNames) statNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (!statValues) statValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        StringBuilder names = new StringBuilder();
        StringBuilder values = new StringBuilder();
        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            names.AppendLine(field.Name);

            object val = field.GetValue(player.Stats);
            float fval = val is int ? (int)val : (float)val;

            PropertyAttribute attribute = (PropertyAttribute)PropertyAttribute.GetCustomAttribute(field, typeof(PropertyAttribute));
            if (attribute != null && field.FieldType == typeof(float))
            {
                float percentage = Mathf.Round(fval * 100 - 100);

                if (Mathf.Approximately(percentage, 0))
                {
                    values.Append("0%").Append('\n');
                }
                else
                {
                    if (percentage > 0)
                        values.Append('+');
                    values.Append(percentage).Append('%').Append('\n');
                }
            }
            else
            {
                values.Append(fval).Append('\n');
            }

            statNames.text = PrettifyNames(names);
            statValues.text = values.ToString();
        }
    }

    public static string PrettifyNames(StringBuilder input)
    {
        string[] polishNames = { "Maksymalne życie", "Regeneracja", "Prędkość ruchu", "Moc", "Prędkość pocisku", "Czas trwania", "Magnes", "Czas odnowy" };

        if (input.Length <= 0) return string.Empty;

        StringBuilder result = new StringBuilder();
        foreach (string name in polishNames)
            result.AppendLine(name);
        // char last = '\0';

        // for (int i = 0; i < input.Length; i++)
        // {
        //     char c = input[i];

        //     if (last == '\0' || char.IsWhiteSpace(last))
        //     {
        //         c = char.ToUpper(c);
        //     }
        //     else if (char.IsUpper(c))
        //     {
        //         result.Append(' ');
        //     }
        //     result.Append(c);

        //     last = c;
        // }

        return result.ToString();
    }

    void Reset()
    {
        player = FindObjectOfType<PlayerStats>();
    }
}
