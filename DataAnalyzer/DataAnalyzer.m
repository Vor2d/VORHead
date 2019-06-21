function [speed_list] = DataAnalyzer(start,endp)
    speed_list = [];
    for i = start:endp
        temp = zeros(1,2);
        temp(1,1) = head.Vel(i,3);
        temp(1,2) = sampleNo(i);
        append(speed_list,temp);
    end
end