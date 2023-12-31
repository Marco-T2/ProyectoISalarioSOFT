PGDMP                     
    {         
   bdplanilla    14.9    14.9 .    ,           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            -           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            .           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            /           1262    24280 
   bdplanilla    DATABASE     g   CREATE DATABASE bdplanilla WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'Spanish_Mexico.1252';
    DROP DATABASE bdplanilla;
                postgres    false            �            1259    24318    aportesdescuento    TABLE     �   CREATE TABLE public.aportesdescuento (
    id integer NOT NULL,
    nombre character varying(10),
    porcentaje numeric(5,2),
    detalle character varying(50)
);
 $   DROP TABLE public.aportesdescuento;
       public         heap    postgres    false            �            1259    24317    aportesdescuento_id_seq    SEQUENCE     �   CREATE SEQUENCE public.aportesdescuento_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 .   DROP SEQUENCE public.aportesdescuento_id_seq;
       public          postgres    false    216            0           0    0    aportesdescuento_id_seq    SEQUENCE OWNED BY     S   ALTER SEQUENCE public.aportesdescuento_id_seq OWNED BY public.aportesdescuento.id;
          public          postgres    false    215            �            1259    24310    diasdescuento    TABLE     �   CREATE TABLE public.diasdescuento (
    id integer NOT NULL,
    idpersona integer,
    fecha date,
    cantdias integer DEFAULT 1,
    descripcion character varying(10) DEFAULT 'FALTA'::character varying
);
 !   DROP TABLE public.diasdescuento;
       public         heap    postgres    false            �            1259    24309    diasdescuento_id_seq    SEQUENCE     �   CREATE SEQUENCE public.diasdescuento_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 +   DROP SEQUENCE public.diasdescuento_id_seq;
       public          postgres    false    214            1           0    0    diasdescuento_id_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE public.diasdescuento_id_seq OWNED BY public.diasdescuento.id;
          public          postgres    false    213            �            1259    24303    dominicalesextra    TABLE     �   CREATE TABLE public.dominicalesextra (
    id integer NOT NULL,
    idpersona integer,
    fecha date,
    cantdias integer DEFAULT 1,
    descripcion character varying(10) DEFAULT 'DOMINICAL'::character varying
);
 $   DROP TABLE public.dominicalesextra;
       public         heap    postgres    false            �            1259    24302    dominicalesextra_id_seq    SEQUENCE     �   CREATE SEQUENCE public.dominicalesextra_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 .   DROP SEQUENCE public.dominicalesextra_id_seq;
       public          postgres    false    212            2           0    0    dominicalesextra_id_seq    SEQUENCE OWNED BY     S   ALTER SEQUENCE public.dominicalesextra_id_seq OWNED BY public.dominicalesextra.id;
          public          postgres    false    211            �            1259    24296 
   horasextra    TABLE     z   CREATE TABLE public.horasextra (
    id integer NOT NULL,
    idpersona integer,
    fecha date,
    canthoras integer
);
    DROP TABLE public.horasextra;
       public         heap    postgres    false            �            1259    24295    horasextra_id_seq    SEQUENCE     �   CREATE SEQUENCE public.horasextra_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 (   DROP SEQUENCE public.horasextra_id_seq;
       public          postgres    false    210            3           0    0    horasextra_id_seq    SEQUENCE OWNED BY     G   ALTER SEQUENCE public.horasextra_id_seq OWNED BY public.horasextra.id;
          public          postgres    false    209            �            1259    24368    personal    TABLE     7  CREATE TABLE public.personal (
    id integer NOT NULL,
    nombre character varying(255),
    nacionalidad character varying(255),
    fechanacimiento date,
    sexo character varying(10),
    fecha_ingreso date DEFAULT CURRENT_DATE,
    cargoocupacion character varying(100),
    diaspagadosmes integer DEFAULT 30,
    horasdiaspagados integer DEFAULT 8,
    haber_basico numeric(10,2),
    afp numeric(8,5) DEFAULT 0.10,
    riesgocomunafp numeric(8,5) DEFAULT 0.0171,
    comisioafp numeric(8,5) DEFAULT 0.0005,
    aportesolidario numeric(8,5) DEFAULT 0.0005
);
    DROP TABLE public.personal;
       public         heap    postgres    false            �            1259    24367    personal_id_seq    SEQUENCE     �   CREATE SEQUENCE public.personal_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 &   DROP SEQUENCE public.personal_id_seq;
       public          postgres    false    220            4           0    0    personal_id_seq    SEQUENCE OWNED BY     C   ALTER SEQUENCE public.personal_id_seq OWNED BY public.personal.id;
          public          postgres    false    219            �            1259    24332 	   productos    TABLE     �   CREATE TABLE public.productos (
    id integer NOT NULL,
    nombre character varying(50),
    precio double precision DEFAULT 5
);
    DROP TABLE public.productos;
       public         heap    postgres    false            �            1259    24331    productos_id_seq    SEQUENCE     �   CREATE SEQUENCE public.productos_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.productos_id_seq;
       public          postgres    false    218            5           0    0    productos_id_seq    SEQUENCE OWNED BY     E   ALTER SEQUENCE public.productos_id_seq OWNED BY public.productos.id;
          public          postgres    false    217            |           2604    24321    aportesdescuento id    DEFAULT     z   ALTER TABLE ONLY public.aportesdescuento ALTER COLUMN id SET DEFAULT nextval('public.aportesdescuento_id_seq'::regclass);
 B   ALTER TABLE public.aportesdescuento ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    216    215    216            y           2604    24313    diasdescuento id    DEFAULT     t   ALTER TABLE ONLY public.diasdescuento ALTER COLUMN id SET DEFAULT nextval('public.diasdescuento_id_seq'::regclass);
 ?   ALTER TABLE public.diasdescuento ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    213    214    214            v           2604    24306    dominicalesextra id    DEFAULT     z   ALTER TABLE ONLY public.dominicalesextra ALTER COLUMN id SET DEFAULT nextval('public.dominicalesextra_id_seq'::regclass);
 B   ALTER TABLE public.dominicalesextra ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    211    212    212            u           2604    24299    horasextra id    DEFAULT     n   ALTER TABLE ONLY public.horasextra ALTER COLUMN id SET DEFAULT nextval('public.horasextra_id_seq'::regclass);
 <   ALTER TABLE public.horasextra ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    209    210    210                       2604    24371    personal id    DEFAULT     j   ALTER TABLE ONLY public.personal ALTER COLUMN id SET DEFAULT nextval('public.personal_id_seq'::regclass);
 :   ALTER TABLE public.personal ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    220    219    220            }           2604    24335    productos id    DEFAULT     l   ALTER TABLE ONLY public.productos ALTER COLUMN id SET DEFAULT nextval('public.productos_id_seq'::regclass);
 ;   ALTER TABLE public.productos ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    218    217    218            %          0    24318    aportesdescuento 
   TABLE DATA           K   COPY public.aportesdescuento (id, nombre, porcentaje, detalle) FROM stdin;
    public          postgres    false    216   .4       #          0    24310    diasdescuento 
   TABLE DATA           T   COPY public.diasdescuento (id, idpersona, fecha, cantdias, descripcion) FROM stdin;
    public          postgres    false    214   �4       !          0    24303    dominicalesextra 
   TABLE DATA           W   COPY public.dominicalesextra (id, idpersona, fecha, cantdias, descripcion) FROM stdin;
    public          postgres    false    212   5                 0    24296 
   horasextra 
   TABLE DATA           E   COPY public.horasextra (id, idpersona, fecha, canthoras) FROM stdin;
    public          postgres    false    210   x5       )          0    24368    personal 
   TABLE DATA           �   COPY public.personal (id, nombre, nacionalidad, fechanacimiento, sexo, fecha_ingreso, cargoocupacion, diaspagadosmes, horasdiaspagados, haber_basico, afp, riesgocomunafp, comisioafp, aportesolidario) FROM stdin;
    public          postgres    false    220   �5       '          0    24332 	   productos 
   TABLE DATA           7   COPY public.productos (id, nombre, precio) FROM stdin;
    public          postgres    false    218   9       6           0    0    aportesdescuento_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.aportesdescuento_id_seq', 7, true);
          public          postgres    false    215            7           0    0    diasdescuento_id_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.diasdescuento_id_seq', 12, true);
          public          postgres    false    213            8           0    0    dominicalesextra_id_seq    SEQUENCE SET     F   SELECT pg_catalog.setval('public.dominicalesextra_id_seq', 12, true);
          public          postgres    false    211            9           0    0    horasextra_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.horasextra_id_seq', 9, true);
          public          postgres    false    209            :           0    0    personal_id_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.personal_id_seq', 20, true);
          public          postgres    false    219            ;           0    0    productos_id_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.productos_id_seq', 5, true);
          public          postgres    false    217            �           2606    24323 &   aportesdescuento aportesdescuento_pkey 
   CONSTRAINT     d   ALTER TABLE ONLY public.aportesdescuento
    ADD CONSTRAINT aportesdescuento_pkey PRIMARY KEY (id);
 P   ALTER TABLE ONLY public.aportesdescuento DROP CONSTRAINT aportesdescuento_pkey;
       public            postgres    false    216            �           2606    24316     diasdescuento diasdescuento_pkey 
   CONSTRAINT     ^   ALTER TABLE ONLY public.diasdescuento
    ADD CONSTRAINT diasdescuento_pkey PRIMARY KEY (id);
 J   ALTER TABLE ONLY public.diasdescuento DROP CONSTRAINT diasdescuento_pkey;
       public            postgres    false    214            �           2606    24308 &   dominicalesextra dominicalesextra_pkey 
   CONSTRAINT     d   ALTER TABLE ONLY public.dominicalesextra
    ADD CONSTRAINT dominicalesextra_pkey PRIMARY KEY (id);
 P   ALTER TABLE ONLY public.dominicalesextra DROP CONSTRAINT dominicalesextra_pkey;
       public            postgres    false    212            �           2606    24301    horasextra horasextra_pkey 
   CONSTRAINT     X   ALTER TABLE ONLY public.horasextra
    ADD CONSTRAINT horasextra_pkey PRIMARY KEY (id);
 D   ALTER TABLE ONLY public.horasextra DROP CONSTRAINT horasextra_pkey;
       public            postgres    false    210            �           2606    24382    personal personal_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.personal
    ADD CONSTRAINT personal_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.personal DROP CONSTRAINT personal_pkey;
       public            postgres    false    220            �           2606    24338    productos productos_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.productos
    ADD CONSTRAINT productos_pkey PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.productos DROP CONSTRAINT productos_pkey;
       public            postgres    false    218            %   }   x�E��
�0���W��F=�Ƕ��^DWl6��?���a�� G�B�mgӯf�8��$��Hd�
��,gݶ��&�5t�	S���]��K���>��1Ao�W���������ao4�      #   E   x�M�!�0@}�K�ܕl���߁aR�M�P�9� �1�aT�^|�8XL�.g[.dy�����)�      !   X   x�}�K
�0��ur�J'��]�n
>.��ϡ�T���@ ɢ	H��c��~�u�YAROm #��:hv�5*Q�,4��P��+~5�         A   x�M��� ��3�b#�bu��?G{���~Apw�Դ�Nÿ89a0��|��~'�	�s�|���      )   -  x����r�0���)��H26x��4M��6I3�N77��
��m��6]�9�b����,ԕ-�t�=:��}�zBWY~�e]������������PX&��$2R	�fJHZ$���9:,*d�`S&�B�#!�8����M�<E"ϑ�p�Fb�iV~A��K�:nȥ�ҟ����8� �a���W��`����U$e$'���,���{�V�4t�70f���Ph���*] ?+��l�9]�m_���<m��\&�vz�s۱E ;aW`��A���b��Can���_�+zr��������4��l^��{���y�i$���8RY?_�kt0�:�:a3p�R����k��?�'�(��/�C�	�H��	�}r�:��$L!m�ʯ��>?��Ҙ,$XC:���e���a;qz���ɒΑ���$Z�eMXԚ���f��D*�j��߱�84���U��A�F��m�S�Dx*|�I�� Q�e�[(WJ�7��)���C5����\�����%��g�?��w�Y�H{�8������ӡ�>��+
2%kN�Z6��Y�~!�=r�e�.6��|
[����mlI�<nG�l�pZ���;�X��w�Pa�k(�^�VԚ7�Vq�}��
E�=[:C�]YSM?����ٲ^�u��ncՕt싊268U28g)�Ew3=jC}�{3ۙ򧹛�W޴���3�h����cC%E��d�+�,鿣��C3ߓ���ԛ,_C��S�����ܧ6�,[jk1�v@�s6�sw1�g���C4�y =�J�qv�}y��q�"z{�'���_J<鮥@����h�m3V]      '   3   x�3�(�O)M.�W0�44�2B��@|c��7A�M@|S�ď���� ��H     