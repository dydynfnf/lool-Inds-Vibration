clc;
clear;
head_length = 46;%46字节时间信息
data_length = 500;%1000字节时间信息
sps = 105500;%采样率
data=[];
fid = fopen('C:\Users\psai\Desktop\新建文件夹\振动4-2017-2-12-15-18-18.bin', 'r');
for i = 1:211;
    fseek(fid,head_length,'cof');
    [temp,count] = fread(fid,500,'short');
    data = [data;temp];
    if count < data_length      %成立时，表示已读取到文件结尾
        break;
    end
end

data=data(1:sps);
%data=data*5/32768;%换电压值
plot(data);

fs=sps;%采样率
N=sps;%fft点数
y=fft(data,N);%fft一半对称
y=abs(y)*2/N;%转换为幅度
y=y(1:N/2);
n=0:N/2-1;
n=n*fs/N;
figure;
plot(n,y);

window=blackmanharris(N);%加窗
data=data.*window;
y=fft(data,N);
y=abs(y)*2/N;
y=y.^2;
y=y(1:N/2);
n=0:N/2-1;
n=n*fs/N;
figure;
plot(n,y,'r');
