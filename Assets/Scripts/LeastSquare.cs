using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeastSquare : MonoBehaviour
{

    //https://www.youtube.com/watch?v=JvS2triCgOY
    protected List<int> x_table;
    protected List<int> y_table;
    private List<int> x_square_table;
    private List<int> xy_table;

    void populateXSquare() {

        for (int i = 0; i < x_table.Count; i++)
        {
            x_square_table.Add(x_table[i] * x_table[i]);
        }
    }

    void populateXY()
    {

        for (int i = 0; i < x_table.Count; i++)
        {
            xy_table.Add(x_table[i] * y_table[i]);
        }
    }

    private int somatoria(List<int> coluna) {
        int result = 0;
        for (int i = 0; i < coluna.Count; i++)
        {
            result =+ coluna[i];
        }

        return result;
    }

    private int media(List<int> coluna) {
        int media = 0;
        
        media = somatoria(coluna) / coluna.Count;

        return media;
    }

   private float correlaçãoLinear()
    {
        float r = 0;

        r = (somatoria(xy_table) - ((somatoria(x_table) * somatoria(y_table)) / x_table.Count)) / ((Mathf.Sqrt(somatoria(x_square_table) - Mathf.Pow(somatoria(x_table), 2) / x_table.Count)) * (Mathf.Sqrt(somatoria(x_square_table) - Mathf.Pow(somatoria(x_table), 2) / x_table.Count)));

        return r;
    }

    private float inclinacaoDaReta() {
        float b = 0;

        b = ((x_table.Count * somatoria(xy_table)) - (somatoria(x_table) * somatoria(y_table))) / (x_table.Count * somatoria(x_square_table) - somatoria(x_square_table));
        return b;
    }

    private float interceptoDaReta() {
        float a = 0;

        a = media(y_table) - inclinacaoDaReta() * media(x_table);
        return a;
    }

    List<float> yLine;
    List<float> yHat() {

        for (int i = 0; i < x_table.Count; i++)
        {
            yLine.Add(interceptoDaReta() + inclinacaoDaReta() * x_table[i]);
        }
        

        return yLine;
    }
    //##############################################
    [ObsoleteAttribute]
    private List<int> desvioDaReta;
    [ObsoleteAttribute]
    private List<int> calculoDeDesvioDaReta(List<int> coluna) {
        
        int mediaValue = media(coluna);

        for (int i = 0; i < coluna.Count; i++)
        {
            desvioDaReta.Add(coluna[i] - mediaValue);
        }

        return desvioDaReta;
    }

    //##############################################

    [ObsoleteAttribute]
    private int determinante() {
        int delta = 0;
        int[,] delta_matriz = new int[2,2];
        delta_matriz[0, 0] = x_table.Count;
        delta_matriz[0, 1] = somatoria(x_table);
        delta_matriz[1, 0] = somatoria(x_table);
        delta_matriz[1, 1] = somatoria(x_square_table);

        delta = (-1 * (delta_matriz[0, 0] * delta_matriz[1, 1]) + (delta_matriz[0, 1] * delta_matriz[1, 0]));

        return delta;
    }
    [ObsoleteAttribute]
    private int determinanteA() {
        int delta = 0;
        int[,] delta_matriz = new int[2, 2];
        delta_matriz[0, 0] = somatoria(y_table);
        delta_matriz[0, 1] = somatoria(xy_table);
        delta_matriz[1, 0] = somatoria(x_table);
        delta_matriz[1, 1] = somatoria(x_square_table);

        delta = (-1 * (delta_matriz[0, 0] * delta_matriz[1, 1]) + (delta_matriz[0, 1] * delta_matriz[1, 0]));

        return delta;
    }
    [ObsoleteAttribute]
    private int determinanteB() {
        int delta = 0;
        int[,] delta_matriz = new int[2, 2];
        delta_matriz[0, 0] = somatoria(y_table);
        delta_matriz[0, 1] = somatoria(xy_table);
        delta_matriz[1, 0] = somatoria(y_table);
        delta_matriz[1, 1] = somatoria(xy_table);

        delta = (-1 * (delta_matriz[0, 0] * delta_matriz[1, 1]) + (delta_matriz[0, 1] * delta_matriz[1, 0]));

        return delta;
    }
    [ObsoleteAttribute]
    private float lastSquareResult() {
        float y = 0, a, b;

        a = determinanteA() / determinante();
        b = determinanteB() / determinante();

        y = a+b;

        return y;
    }
}
