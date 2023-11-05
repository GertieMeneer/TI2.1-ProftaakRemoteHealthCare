using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace T2._1_HealthCareDokter.Graphs
{
    public class Graph<T>
    {
        private LineSeries _lineSeries;
        
        private List<T> _values = new List<T>();
        private ChartValues<T> _chartValues = new ChartValues<T>();

        public Graph(CartesianChart cartesianChart)
        {
            _lineSeries = new LineSeries(){Values = _chartValues};
            var seriesCollection = new SeriesCollection(){_lineSeries};
            
            cartesianChart.Series = seriesCollection;
            // cartesianChart.AxisX.Add(new Axis());
        }
        
        public void AddValue(T value)
        {
            _chartValues.Add(value);
            _values.Add(value);
        }
        
        public void RemoveValue(T value)
        {
            _chartValues.Remove(value);
            _values.Remove(value);
        }
        
        public void RemoveValueAt(int index)
        {
            _chartValues.RemoveAt(index);
            _values.RemoveAt(index);
        }
        
        public void Clear()
        {
            _chartValues.Clear();
            _values.Clear();
        }
        
        public void SetChartToSetValues(int index, int endIndex)
        {
            _chartValues.Clear();
            if (index < 0) index = 0;
            if (endIndex > _values.Count - 1) endIndex = _values.Count - 1;
            for (int i = index; i <= endIndex; i++)
            {
                _chartValues.Add(_values[i]);
            }
        }
        
        
        public void SetChartToLast20()
        {
            _chartValues.Clear();
            var index = _values.Count - 20;
            if (index < 0) index = 0;
            for (int i = index; i < _values.Count; i++)
            {
                _chartValues.Add(_values[i]);
            }
        }
        
        public int GetCount()
        {
            return _values.Count;
        }
        
        public Graph<T> SetLineStyle(double lineSmoothness)
        {
            _lineSeries.LineSmoothness = lineSmoothness;
            return this;
        }
        
        public Graph<T> SetLineColor(SolidColorBrush color)
        {
            _lineSeries.Stroke = color;
            return this;
        }
        
    }
}