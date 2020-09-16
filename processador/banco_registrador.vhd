library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;


entity BANCO_REGISTRADOR is
    Generic (
					p_DATA_WIDTH		: INTEGER := 16
	 );
    Port ( 
				i_CLK   		: in  STD_LOGIC;
				i_RST   		: in  STD_LOGIC;
				
				i_ADD_S1		: in  STD_LOGIC_VECTOR(1 DOWNTO 0);	  
				i_ADD_S2		: in  STD_LOGIC_VECTOR(1 DOWNTO 0);	

				i_RET			: in  STD_LOGIC;
				i_DOUT_LIFO 	: in  STD_LOGIC_VECTOR(63 DOWNTO 0);
				o_DIN_LIFO  	: out STD_LOGIC_VECTOR(63 DOWNTO 0);		
				
				i_WR_IO       	: in  STD_LOGIC;
				i_WR_RAM 		: in  STD_LOGIC;
				i_WR  			: in  STD_LOGIC;
				i_DATA			: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
			    i_ADD_DST 		: in  STD_LOGIC_VECTOR(1 DOWNTO 0);
				o_RS1 			: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
				o_RS2 			: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0)
	 );
end BANCO_REGISTRADOR;

architecture Behavioral of BANCO_REGISTRADOR is

	COMPONENT REGISTRADOR is
		 Generic (
					p_DATA_WIDTH		: INTEGER := 16
		 );
		 Port ( 
					i_CLK   : in  STD_LOGIC;
					i_RST   : in  STD_LOGIC;
					i_DATA	: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	  
					o_DATA	: out STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	
					i_WR   	: in  STD_LOGIC	
		 );
	end COMPONENT;	
	
	
	signal	w_DOUT00		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal	w_DOUT01		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);

	signal	w_DOUT02		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal	w_DOUT03		: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	
	signal   w_WR1			: STD_LOGIC;
	signal   w_WR2			: STD_LOGIC;
	signal   w_WR3			: STD_LOGIC;
	signal   w_WR4			: STD_LOGIC;
	signal   w_REG0     	: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_REG1     	: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_REG2     	: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	signal   w_REG3     	: STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);
	
begin
	--
	--- Salvamento e restauracao de registradores.
	--

	o_DIN_LIFO 	<= w_DOUT03 & w_DOUT02 & w_DOUT01 & w_DOUT00;
		
	w_REG0 		<= i_DOUT_LIFO(15 downto 0) when (i_RET = '1') else i_DATA;	
	
	U00:  REGISTRADOR 
    Port map( 
					i_CLK   => i_CLK,
					i_RST   => i_RST,
					i_DATA	=> w_REG0,
					o_DATA	=> w_DOUT00,
					i_WR   	=> w_WR1
	 );

 	w_REG1 <= i_DOUT_LIFO(31 downto 16) when (i_RET = '1') else i_DATA;

	 U01:  REGISTRADOR 
    Port map( 
					i_CLK   => i_CLK,
					i_RST   => i_RST,
					i_DATA	=> w_REG1,
					o_DATA	=> w_DOUT01,
					i_WR   	=> w_WR2
	 );

	w_REG2 <= i_DOUT_LIFO(47 downto 32) when (i_RET = '1') else i_DATA;
	 
	U02:  REGISTRADOR 
    Port map( 
					i_CLK   => i_CLK,
					i_RST   => i_RST,
					i_DATA	=> w_REG2,
					o_DATA	=> w_DOUT02,
					i_WR   	=> w_WR3
	 );

	 
	 w_REG3 <= i_DOUT_LIFO(63 downto 48) when (i_RET = '1') else i_DATA;
		
	 U03:  REGISTRADOR 
    Port map( 
					i_CLK   => i_CLK,
					i_RST   => i_RST,
					i_DATA	=> w_REG3,
					o_DATA	=> w_DOUT03,
					i_WR   	=> w_WR4
	 );	 

	 
	 
	 MUX_S1 : PROCESS(i_ADD_S1, i_ADD_DST, i_WR_RAM, w_DOUT00, w_DOUT01, w_DOUT02, w_DOUT03, i_WR_IO)
	 BEGIN
			IF ((i_ADD_S1 = "00") AND (i_WR_RAM = '0') AND (i_WR_IO = '0'))THEN
				o_RS1 <= w_DOUT00;
			ELSIF ((i_ADD_S1 = "01") AND (i_WR_RAM = '0') AND (i_WR_IO = '0')) THEN
				o_RS1 <= w_DOUT01;
			ELSIF ((i_ADD_S1 = "10") AND (i_WR_RAM = '0') AND (i_WR_IO = '0')) THEN
				o_RS1 <= w_DOUT02;
			ELSIF ((i_ADD_S1 = "11") AND (i_WR_RAM = '0') AND (i_WR_IO = '0')) THEN
				o_RS1 <= w_DOUT03;
				
			ELSIF ((i_ADD_DST = "00") AND (i_WR_RAM = '1') AND (i_WR_IO = '0'))THEN
				o_RS1 <= w_DOUT00;
			ELSIF ((i_ADD_DST = "01") AND (i_WR_RAM = '1') AND (i_WR_IO = '0')) THEN
				o_RS1 <= w_DOUT01;
			ELSIF ((i_ADD_DST = "10") AND (i_WR_RAM = '1') AND (i_WR_IO = '0')) THEN
				o_RS1 <= w_DOUT02;
			ELSIF ((i_ADD_DST = "11") AND (i_WR_RAM = '1') AND (i_WR_IO = '0')) THEN
				o_RS1 <= w_DOUT03;		
				
			ELSIF ((i_ADD_DST = "00") AND (i_WR_RAM = '0') AND (i_WR_IO = '1'))THEN
				o_RS1 <= w_DOUT00;
			ELSIF ((i_ADD_DST = "01") AND (i_WR_RAM = '0') AND (i_WR_IO = '1')) THEN
				o_RS1 <= w_DOUT01;
			ELSIF ((i_ADD_DST = "10") AND (i_WR_RAM = '0') AND (i_WR_IO = '1')) THEN
				o_RS1 <= w_DOUT02;
			ELSIF ((i_ADD_DST = "11") AND (i_WR_RAM = '0') AND (i_WR_IO = '1')) THEN
				o_RS1 <= w_DOUT03;		
				
			ELSE
				o_RS1 <= w_DOUT00;
			END IF;
	 END PROCESS MUX_S1;

	 
	 MUX_S2 : PROCESS(i_ADD_S2, w_DOUT00, w_DOUT01, w_DOUT02, w_DOUT03)
	 BEGIN
			IF (i_ADD_S2 = "00") THEN
				o_RS2 <= w_DOUT00;
			ELSIF (i_ADD_S2 = "01") THEN
				o_RS2 <= w_DOUT01;
			ELSIF (i_ADD_S2 = "10") THEN
				o_RS2 <= w_DOUT02;
			ELSE
				o_RS2 <= w_DOUT03;
			END IF;
	 END PROCESS MUX_S2;
	 
	 w_WR1 <= ((NOT i_ADD_DST(0)) AND (NOT i_ADD_DST(1)) AND i_WR) OR i_RET; 
	 w_WR2 <= (     i_ADD_DST(0)  AND (NOT i_ADD_DST(1)) AND i_WR) OR i_RET; 
 	 w_WR3 <= ((NOT i_ADD_DST(0)) AND      i_ADD_DST(1)  AND i_WR) OR i_RET; 
 	 w_WR4 <= (     i_ADD_DST(0)  AND      i_ADD_DST(1)  AND i_WR) OR i_RET; 
  
	 
end behavioral;
	