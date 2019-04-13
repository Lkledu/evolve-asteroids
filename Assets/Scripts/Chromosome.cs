﻿using System;
using System.Text;

public class Chromosome : IComparable<Chromosome>, ICloneable
{
    private static readonly int MAX_ALLELE = 12;

    public int[] Genes { get; private set; }

    public int this[int index]
    {
        get { return Genes[index]; }
        set { Genes[index] = value; }
    }

    public int Length
    {
        get { return Genes.Length; }
    }

    public double Fitness { get; set; }

    public Chromosome(int length)
    {
        Genes = new int[length];
        for (int i = 0; i < Length; i++)
        {
            Genes[i] = Helper.NextInt(MAX_ALLELE);
        }

        Fitness = 0.0f;
    }

    public Chromosome Crossover(Chromosome otherParent, double crossoverRate)
    {
        Chromosome child = new Chromosome(Length);
        for (int i = 0; i < child.Length; i++)
        {
            child[i] = Helper.NextDouble() < crossoverRate ? Genes[i] : otherParent[i];
        }

        return child;
    }

    public void Mutate(double mutationRate)
    {
        for (int i = 0; i < Length; i++)
        {
            Genes[i] = Helper.NextDouble() < mutationRate ? Helper.NextInt(MAX_ALLELE) : Genes[i];
        }
    }

    public object Clone()
    {
        Chromosome chromosome = new Chromosome(Length);

        chromosome.Fitness = Fitness;
        Array.Copy(Genes, chromosome.Genes, Length);

        return chromosome;
    }

    #region [ Overrides ]
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Chromosome chromosome = (Chromosome)obj;
        for (int i = 0; i < Length; i++)
        {
            if (!Genes[i].Equals(chromosome[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (int alelo in Genes)
            {
                hash = hash * alelo.GetHashCode();
            }

            return hash;
        }
    }

    public int CompareTo(Chromosome other)
    {
        if (Fitness > other.Fitness)
        {
            return -1;
        }

        if (Fitness < other.Fitness)
        {
            return 1;
        }

        return 0;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[");
        Array.ForEach(Genes, x => builder.Append(x).Append("|"));
        builder.Remove(builder.Length - 1, 1);
        builder.Append("] = ").Append(Fitness);
        return builder.ToString();
    }
    #endregion
}