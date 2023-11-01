using System;

public class TsvParser
{
	private string filePath;
	private string qualitativeVariable;
	private string quantitativeVariableD;
	private string quantitativeVariableC;
	

    public TsvParser(string filePath, string qualitativeVariable, string quantitativeVariableD, string quantitativeVariableC)
	{
		this.SetPath(filePath);
	}
	
	private void SetPath(string filePath)
	{
		this.filePath = filePath;
	}

	private void SetQualitativeVariable(string qualitativeVariable)
	{
		this.qualitativeVariable = qualitativeVariable;
	}

    private void SetQuantitativeVariableD(string quantitativeVariableD)
    {
		this.quantitativeVariableD = quantitativeVariableD;
    }

    private void SetQuantitativeVariableC(string quantitativeVariableC)
    {
        this.quantitativeVariableC = quantitativeVariableC;
    }


    public string getPath()
	{
		return this.filePath;
	}

	public void FileParser(string filePath)
	{
		StreamReader sr = new StreamReader(filePath);
		char[] delimiter = new char[] { '\t' };
		string[] columnheader = sr.ReadLine().Split(delimiter);
		string[] dataChosen;
		foreach(string i in columnheader)
		{
			if(i.Equals(qualitativeVariable) || i.Equals(quantitativeVariableD) || i.Equals(quantitativeVariableC)){
                dataChosen[i] = i;
			}
		}
	}
}
