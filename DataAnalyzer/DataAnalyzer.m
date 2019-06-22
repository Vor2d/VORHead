function [speed_list] = DataAnalyzer(start,endp,data)
    speed_list = cell(endp-start,2);
    for i = start:endp
        temp = zeros(1,2);
        temp(1,1) = data.head.Vel(i,3);
        temp(1,2) = data.sampleNo(i);
        speed_list{i,1} = temp(1,2);
        speed_list{i,2} = temp(1,1);
    end
end