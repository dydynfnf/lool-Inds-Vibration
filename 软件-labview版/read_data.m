clc;
clear;
head_length = 46;%46�ֽ�ʱ����Ϣ
data_length = 500;%1000�ֽ�ʱ����Ϣ
sps = 105500;%������
data=[];
fid = fopen('C:\Users\psai\Desktop\�½��ļ���\��4-2017-2-12-15-18-18.bin', 'r');
for i = 1:211;
    fseek(fid,head_length,'cof');
    [temp,count] = fread(fid,500,'short');
    data = [data;temp];
    if count < data_length      %����ʱ����ʾ�Ѷ�ȡ���ļ���β
        break;
    end
end

data=data(1:sps);
%data=data*5/32768;%����ѹֵ
plot(data);

fs=sps;%������
N=sps;%fft����
y=fft(data,N);%fftһ��Գ�
y=abs(y)*2/N;%ת��Ϊ����
y=y(1:N/2);
n=0:N/2-1;
n=n*fs/N;
figure;
plot(n,y);

window=blackmanharris(N);%�Ӵ�
data=data.*window;
y=fft(data,N);
y=abs(y)*2/N;
y=y.^2;
y=y(1:N/2);
n=0:N/2-1;
n=n*fs/N;
figure;
plot(n,y,'r');
